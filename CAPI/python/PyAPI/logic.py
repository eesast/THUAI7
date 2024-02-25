import os
from typing import List, Union, Callable, Tuple
import threading
import logging
import copy
import platform
import proto.MessageType_pb2 as MessageType
import proto.Message2Server_pb2 as Message2Server
import proto.Message2Clients_pb2 as Message2Clients
from queue import Queue
import PyAPI.structures as THUAI6
from PyAPI.utils import Proto2THUAI6, AssistFunction
from PyAPI.API import ShipAPI, TeamAPI
from PyAPI.AI import Setting
from PyAPI.Communication import Communication
from PyAPI.State import State
from PyAPI.Interface import ILogic, IGameTimer