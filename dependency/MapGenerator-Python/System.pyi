#!/usr/bin/env python
# -*- coding: utf-8 -*-

# Stub for C#

from __future__ import annotations


class UInt32:
    def __init__(self, i: int) -> None: ...


class String:
    def __init__(self, s: str) -> None: ...


class Array[T]:
    @staticmethod
    def CreateInstance(T: type, h: int, w: int) -> Array[T]: ...
