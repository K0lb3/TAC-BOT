#This is the page item template. this will be called first then the data will be rendered into this object
#This will be inherited by all other objects
import discord

class Embed:
    title='Noctis'
    url='http://www.google.com'
    

    def getEmbed(self): #returns the embed form of this object
        embed = discord.Embed(
            title="general info,overall rank: [SS]",
            url="http://www.google.com"
            )
        embed.set_author(name='Noctis',url='http://www.google.com')
        embed.set_thumbnail(url='http://cdn.alchemistcodedb.com/images/units/profiles/ff15_noct.png')
        embed.set_image(url="http://cdn.alchemistcodedb.com/images/units/artworks/ff15_noct-closeup.png")
        embed.add_field(name="Gender",value="♂")
        embed.add_field(name="Rarity",value="★★★★★")
        embed.add_field(name="Country",value="The Kingdom of Lucis")
        embed.add_field(name="Collab",value="Final Fantasy 15")
        embed.add_field(name="Leader Skill",value="Water:+40% HP,-10 Cast Time Ratio")
        embed.add_field(name="Job 1",value="Prince of Lucis[S]")
        embed.add_field(name="Job 2",value="Magic Swordsman[A]")
        embed.add_field(name="Job 3",value="Battle Mage[SS]")
        return embed
        
        
    
