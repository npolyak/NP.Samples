from google.protobuf.internal import enum_type_wrapper as _enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor
None: Topic
OrgTopic: Topic

class Org(_message.Message):
    __slots__ = ["Name", "NumberPeople"]
    NAME_FIELD_NUMBER: _ClassVar[int]
    NUMBERPEOPLE_FIELD_NUMBER: _ClassVar[int]
    Name: str
    NumberPeople: int
    def __init__(self, Name: _Optional[str] = ..., NumberPeople: _Optional[int] = ...) -> None: ...

class Topic(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
    __slots__ = []
