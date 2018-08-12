from .abstractEmbed import Embed

class EmbedUnit(Embed):
    
    def __init__(self,Unit):
        self.title = Unit.name
