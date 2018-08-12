from .abstractAsset import Asset


class Unit(Asset):
    def __init__(self,name):
        self.name = name
