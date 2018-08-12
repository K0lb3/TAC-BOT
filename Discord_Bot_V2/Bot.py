import os
import discord
import asyncio
import json
from discord.ext import commands
from functions import *
from settings import *
from model.Assets import *
from model.EmbedPages import *
from model.Repository import *

# Constants
bot = commands.Bot(command_prefix=prefix)

main = LoadUnits()
logi = (main['UN_V2_LOGI'])
#print (logi)
r = Repository()
r.repo.set('UN_V2_LOGI',logi)
value = r.repo.get('UN_V2_LOGI')
print(value)

#we shall move the resources to a redis server here
#test commands
#r = Repository()
#r.repo.set('foo','bar')
#value = r.repo.get('foo')
#print(value)



#some stats stuff
async def statistic(ctx, command, input=False, result=False): 
    Channel = bot.get_channel(457645688583618560)
    embed=discord.Embed(title='Command: '+command,color=0x00FF00)
    embed.add_field(name="Guild",   value=ctx.guild)
    embed.add_field(name="User",    value=ctx.author)
    if input:
        embed.add_field(name="Input",   value=input)
    if result:
        embed.add_field(name="Result",  value=result)
    await Channel.send(embed=embed)


#setting bot funny names
async def status_task(presences):
    while True:
        for p in presences:
            game = discord.Game(p)
            await bot.change_presence(status=discord.Status.online, activity=game)
            await asyncio.sleep(10)

#function to add reactions
async def add_reactions(msg,reactions):
    for r in reactions:
        await msg.add_reaction(r)

#when bot is initialized this runs
@bot.event
async def on_ready():
    print('Logged in as')
    print(bot.user.name)
    print(bot.user.id)
    print(bot.guilds)
    print('------')
    bot.loop.create_task(status_task(PRESENCES))


@bot.command() 
async def unit(ctx,name): #returns the required unit page or a nice not found error
    #1. Get the Unit Object
    unit = Unit(name);
    #2. Adds the unit object into the page
    embedPage = EmbedUnit(unit)
    #3. sends page over
    await ctx.send(embed=embedPage.getEmbed())


bot.run(BOT_TOKEN)
