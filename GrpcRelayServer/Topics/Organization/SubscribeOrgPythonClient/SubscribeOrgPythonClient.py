# (c) Nick Polyak 2023
# License: MIT License (https://opensource.org/licenses/MIT)

from RelayService_pb2 import FullMsg
from ObservingRelayClient import ObservingRelayClient
from Org_pb2 import Org

observingClient = ObservingRelayClient("localhost", 5555)

observingClient.observe_topic(topic_name="OrgTopic", topic_number=20)

org_observable = observingClient.get_concrete_observable(lambda:Org())

org_observable.subscribe(on_next = lambda o:print("\n" + o.Name))

input("Subscribing client! Press any key to exit!")