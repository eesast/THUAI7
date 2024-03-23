from collections import deque, namedtuple
import random
Transition = namedtuple("Transition", ["state", "action", "reward", "next_state"])
class Memory:
    def __init__(self,max_len):
        self.memory=deque([],maxlen=max_len)
    def push(self,*args):
        self.memory.append(Transition(*args))
    def sample(self,batch_size):
        return random.sample(self.memory,batch_size)
    def __len__(self):
        return len(self.memory)