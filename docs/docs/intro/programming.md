# 编程

import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

## Visual Studio 使用说明

比赛**只保证！！！支持** VS 2022 最新版本，选手使用其他版本后果自负（实际上应该不能编译）。

### 生成模式的设置

菜单栏下方一行

![image-20230416010705076](https://raw.githubusercontent.com/shangfengh/THUAI6/new/resource/image-20230416010705076.png)

可以更改生成模式为 `Debug` 或 `Release`

### 命令行参数的设置

左上方菜单栏 `调试->调试属性`

![image-20230416010816392](https://raw.githubusercontent.com/shangfengh/THUAI6/new/resource/image-20230416010816392.png)

在命令参数一栏中加入命令行参数进行调试

### cmd 脚本的参数修改

右键点击 `.cmd` 或 `.bat` 文件之后，选择编辑就可以开始修改文件。通过在一行的开头加上 `::`，可以注释掉该行。

## 接口必看

<Tabs>
<TabItem value="cpp" label="C++" default>

**在此鸣谢\xfgg/\xfgg/\xfgg/，看到这里的选手可以到选手群膜一膜！！！ **

除非特殊指明，以下代码均在 MSVC 19.28.29913 x64 `/std:cpp17` 与 GCC 10.2 x86_64-linux-gnu `-std=cpp17` 两个平台下通过。

由于我们的比赛最终会运行在 Linux 平台上，因此程设课上学到的一些只适用于 Windows 的 C++ 操作很可能并不能正确执行。此外，代码中使用了大量 Modern C++ 中的新特性，可能会使选手在编程过程中遇到较大困难。因此，此处介绍一些比赛中使用 C++ 接口必须了解的知识。

### 计时相关

编写代码过程中，我们可能需要获取系统时间等一系列操作，C++ 标准库提供了这样的行为。尤其注意**不要**使用 Windows 平台上的 `GetTickCount` 或者 `GetTickCount64` ！！！应当使用 `std::chrono`

头文件：`#include <chrono>`

可以用于获取时间戳，从而用于计时、例如计算某个操作花费的时间，或者协调队友间的合作。

```cpp
#include <iostream>
#include <chrono>
int main()
{
    auto sec = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    std::cout << "从 1970 年元旦到现在的：秒数" << sec << "；毫秒数：" <<
         msec << std::endl;
    return 0;
}
```

### 线程睡眠

由于移动过程中会阻塞人物角色，因此玩家可能要在移动后让线程休眠一段时间，直到移动结束。C++ 标准库中使线程休眠需要包含头文件：`#include <thread>`。示例用法：

我们推荐小步移动，不太建议玩家使用线程睡眠超过一帧

```cpp
std::this_thread::sleep_for(std::chrono::milliseconds(20));     // 休眠 20 毫秒
std::this_thread::sleep_for(std::chrono::seconds(2));           // 休眠 2 秒

// 下面这个也能休眠 200 毫秒
std::this_thread::sleep_until(std::chrono::system_clock::now()
    += std::chrono::milliseconds(200));
```

休眠过程中，线程将被阻塞，而不继续进行，直到休眠时间结束方继续向下执行。

### 异步接口的使用

本届比赛中，我们可能会看到类似 `std::future<bool>` 这样类型的接口返回值，这实际上是一个异步接口。在调用同步接口后，在接口内的函数未执行完之前，线程通常会阻塞住；但是异步接口的调用通常不会阻塞当前线程，而是会另外开启一个线程进行操作，当前线程则继续向下执行。当调用 `get()` 方法时，将返回异步接口的值，若此时异步接口内的函数依然未执行完，则会阻塞当前线程。

如果不需要返回值或没有返回值，但是希望接口内的函数执行完之后再进行下一步，即将接口当做常规的同步接口来调用，也可以调用 `wait()` 方法。

```cpp
#include <iostream>
#include <thread>
#include <future>
#include <chrono>

int f_sync()
{
    std::this_thread::sleep_for(std::chrono::seconds(1));
    return 8;
}

std::future<int> f_async()
{
    return std::async(std::launch::async, []()
                      { std::this_thread::sleep_for(std::chrono::seconds(1));
                        return 8; });
}

int main()
{
    auto start = std::chrono::system_clock::now();
    std::cout << std::chrono::duration_cast<std::chrono::duration<double, std::milli>>(
        std::chrono::system_clock::now() - start).count() << std::endl;
    auto x = f_async();
    std::cout << std::chrono::duration_cast<std::chrono::duration<double, std::milli>>(
        std::chrono::system_clock::now() - start).count() << std::endl;
    std::cout << x.get() << std::endl;
    std::cout << std::chrono::duration_cast<std::chrono::duration<double, std::milli>>(
        std::chrono::system_clock::now() - start).count() << std::endl;
    auto y = f_sync();
    std::cout << std::chrono::duration_cast<std::chrono::duration<double, std::milli>>(
        std::chrono::system_clock::now() - start).count() << std::endl;
    std::cout << y << std::endl;
    std::cout << std::chrono::duration_cast<std::chrono::duration<double, std::milli>>(
        std::chrono::system_clock::now() - start).count() << std::endl;
}
```

### `auto` 类型推导

C++11开始支持使用 `auto` 自动推导变量类型，废除了原有的作为 storage-class-specifier 的作用：

```cpp
int i = 4;
auto x = i; // auto 被推导为 int，x 是 int 类型
auto& y = i; // auto 仍被推导为 int，y 是 int& 类型
auto&& z = i; // auto 被推导为 int&，z 是 int&&&，被折叠为 int&，即 z 与 y 同类型
auto&& w = 4; // auto 被推导为 int，w 是 int&& 类型
```

### STL相关

#### `std::vector`

头文件：`#include <vector>`，类似于可变长的数组，支持下标运算符 `[]` 访问其元素，此时与 C 风格数组用法相似。支持 `size` 成员函数获取其中的元素数量。

创建一个 `int` 型的 `vector`  对象：

```cpp
std::vector<int> v { 9, 1, 2, 3, 4 };   // 初始化 vector 有五个元素，v[0] = 9, ...
v.emplace_back(10);         // 向 v 尾部添加一个元素，该元素饿构造函数的参数为 10（对于 int，只有一个语法意义上的构造函数，无真正的构造函数），即现在 v 有六个元素，v[5] 的值是 10
v.pop_back();               // 把最后一个元素删除，现在 v 还是 { 9, 1, 2, 3, 4 }
```

遍历其中所有元素的方式：

```cpp
// std::vector<int> v;
for (int i = 0; i < (int)v.size(); ++i)
{
    /*可以通过 v[i] 对其进行访问*/
}

for (auto itr = v.begin(); itr != v.end(); ++itr)
{
    /*
    * itr 作为迭代器，可以通过其访问 vector 中的元素。其用法与指针几乎完全相同。
    * 可以通过 *itr 得到元素；以及 itr-> 的用法也是支持的
    * 实际上它内部就是封装了指向 vector 中元素的指针
    * 此外还有 v.cbegin()、v.rbegin()、v.crbegin() 等
    * v.begin()、v.end() 也可写为 begin(v)、end(v)
    */
}

for (auto&& elem : v)
{
    /*
    * elem 即是 v 中每个元素的引用，也可写成 auto& elem : v
    * 它完全等价于：
    * {
    *   auto&& __range = v;
    *   auto&& __begin = begin(v);
    *   auto&& __end = end(v);
    *   for (; __begin != __end; ++__begin)
    *   {
    *       auto&& elem = *__begin;
    *       // Some code
    *   }
    * }
    */
}
```

例如：

```cpp
for (auto elem&& : v) { std::cout << elem << ' '; }
std::cout << std::endl;
```

作为 STL 的容器之一，其具有容器的通用接口。但是由于这比较复杂，在此难以一一展开。有兴趣的同学可以在下方提供的链接里进行查阅。

**注：请千万不要试图使用 `std::vector<bool>`，若需使用，请用 `std::vector<char>` 代替！**

更多用法参见（点击进入）：[cppreference_vector](https://zh.cppreference.com/w/cpp/container/vector)

#### std::array

头文件：`#include <array>`，C 风格数组的类封装版本。

用法与 C 风格的数组是基本相似的，例如：

```cpp
std::array<double, 5> arr { 9.0, 8.0, 7.0, 6.0, 5.0 };
std::cout << arr[2] << std::endl;   // 输出 7.0
```

同时也支持各种容器操作：

```cpp
double sum = 0.0;
for (auto itr = begin(arr); itr != end(arr); ++itr)
{
    sum += *itr;
}
// sum 结果是 35
```

更多用法参见（点击进入）：[cppreference_array](https://zh.cppreference.com/w/cpp/container/array)。

### fmt 库的使用

选手包中内置了 `fmt` 库，因此选手可以通过 `fmt` 库来格式化字符串，而无须自己手动格式化。

```cpp
std::string str_std = "number: " + std::to_string(1) + teststr;
std::string str_fmt = fmt::format("number: {}{}", 1, teststr); // 两种方法等价
```

`fmt`库还支持更多强大的操作，例如更精确的格式化、更简便的转化等。

更多用法参见（点击进入）：[fmt_index](https://fmt.dev/latest/index.html)。

</TabItem>
<TabItem value="python" label="Python">

比赛**只保证！！**支持 Python 3.9，不保证支持其他版本

比赛中的 Python 接口大多使用异步接口，即返回一个类似于 `Future[bool]` 的值。为了获取实际的值，需要调用 `result()` 方法。

```python
from concurrent.futures import Future, ThreadPoolExecutor
import time

class Cls:
    def __init__(self):
        self.__pool: ThreadPoolExecutor = ThreadPoolExecutor(10)

    def Test(self, a: int, b: int) -> Future[int]:
        def test():
            time.sleep(0.5)
            return a + b

        return self.__pool.submit(test)

if __name__ == '__main__':
    f1 = Cls().Test(1, 2)
    print(time.time())
    print(f1.result())
    print(time.time())

```

</TabItem>
</Tabs>