# (c) Nick Polyak 2023
# License: MIT License (https://opensource.org/licenses/MIT)

from RelayService_pb2 import FullMsg
from BroadcastingRelayClient import BroadcastingRelayClient
from Org_pb2 import Org

print("Broadcasting org")

broadcastingClient = BroadcastingRelayClient("localhost", 5555)

broadcastingClient.connect_if_needed()

o = Org()
o.Name = "Great and powerful Org"

broadcastingClient.broadcast_object(o, topic_name="OrgTopic", topic_number=20)
