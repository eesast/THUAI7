# MapGenerator开发指南

- [MapGenerator开发指南](#mapgenerator开发指南)
  - [虚拟环境](#虚拟环境)
  - [代码说明](#代码说明)
  - [构建项目](#构建项目)
  - [其他](#其他)

## 虚拟环境

本项目使用 `pipenv`[^1] 管理虚拟环境.

`Pipfile` 是关于虚拟环境的信息, 包括 `pip` 的信息、项目使用的第三方库和项目的 Python 版本.

`Pipfile.lock`  类似于传统的 `requirements.txt`, 用于确保依赖版本的适配. 使用如下指令可以按照 `Pipfile.lock` 安装依赖:

```shell
pipenv install          # 从Pipfile安装
pipenv install --dev    # 安装为仅开发
pipenv sync             # 从Pipfile.lock安装
```

向环境中加入依赖, 使用:

```shell
pipenv install package              # 安装依赖
pipenv install --dev dev_package    # 仅开发安装
```

删除依赖, 需要使用:

```shell
pipenv uninstall your_package
```

在虚拟环境中运行脚本, 使用如下指令:

```shell
pipenv run python3 main.py
```

## 代码说明

`./CLR_START.py`、`./CLR_IMPORT.py` 为使用 PythonNet 加载 CLR 的脚本. 在 `./CLR_IMPORT.py` 中有一串 `GameClass` 和 `Preparation` 路径, 是所需的 C# DLL 的位置. 如果出现找不到 DLL 的问题, 请检查 C# 是否编译以及编译产物的路径.

`./GameClass/MapGenerator.pyi`、`./Preparation/Utility.pyi` 和 `./System.pyi` 为 C# DLL 的调用存根, 用于语法高亮; 不需要语法高亮请无视或删除.

> 本项目使用了 C# 的 `GameClass.MapGenerator.MapStruct` 和 `Preparation.Utility.PlaceType`, 以及 C# 的基础类型 `UInt32`、`string` 和 `Array`.
> PythonNet 可以自动将数字和字符串类型在 Python 和 Dotnet 之间转换.

`./SETTINGS.py` 为设置脚本, 分别为程序窗口标题、地图文件后缀名和地图区域类型所对应的颜色.

`./Classes/MapRenderer.py` 为程序主体, 负责提供 UI 界面和地图渲染及编辑. 其中存在 `on_click()`(鼠标事件侦测) 和 `on_press()`(键盘事件侦测), 可按需修改.

`./Classes/RandomCore.py` 为 `RandomCore` 虚基类的所在.

`./Classes/RandomCores/` 为 `RandomCore` 的实现.

> 注意: `RandomCore` 的实现具有强的项目依赖性, 请根据项目实际情况重写.

`./main.py` 为启动脚本. 其中需要留意的是 `RandomCore` 的注册——将其改为重写后的实现.

## 构建项目

在代码根据项目实际修改完成后, 先编译依赖 C# DLL.

C# DLL 编译完成后, 再次检查 DLL 的路径, 包括 `./CLR_IMPORT.py` 的路径和 `./MapGenerator.spec` 里 `datas` 参数的路径.

检查完成后, 运行下列命令即可:

```shell
# 首次编译
pipenv run pyinstaller ./MapGenerator.spec
# 如果项目编译过一次了, 用这个跳过文件夹清除询问
pipenv run pyinstaller ./MapGenerator.spec -y
```

项目编译在 `./dist/`下.

## 其他

`./pyproject.toml` 目前用于规定 autopep8 代码格式化参数. 想要格式化代码, 只需:

```shell
autopep8 .
```

[^1]: https://pipenv.pypa.io/en/latest/cli.html