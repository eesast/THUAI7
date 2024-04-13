import os
import sys
import grpc

sys.path.append(os.path.dirname(os.path.realpath(__file__)) + "/proto")

import proto.Message2Server_pb2 as m2s  # NOQA: E402
import proto.Message2Clients_pb2 as m2c  # NOQA: E402
import proto.MessageType_pb2 as mt  # NOQA: E402
import proto.Services_pb2_grpc as ser  # NOQA: E402

stub = ser.AvailableServiceStub(grpc.insecure_channel('localhost:8888'))
for msg in stub.AddPlayer(m2s.PlayerMsg(player_id=2025)):
    print(msg)
