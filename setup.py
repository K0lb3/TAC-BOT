# -*- coding: utf-8 -*-
from setuptools import setup, find_packages
from os import path

here = path.abspath(path.dirname(__file__))

try:
    with open(path.join(here, 'README.md')) as f:
        long_description = f.read()
except Exception:
    # XXX: Intentional pokemon catch to prevent this read from breaking setup.py
    long_description = None

about = {}
with open(path.join(here, "Discord_Bot_V2", "__version__.py")) as f:
    exec(f.read(), about)


setup(
    name='tac_bot_db',
    version=about['__version__'],
    author='@K0lb3',
    description='Unofficial Discord bot for The Alchemist Code',
    long_description=long_description,
    url='https://github.com/K0lb3/TAC-BOT',
    packages=find_packages(exclude=['tests', 'discord_bot', 'discord_bot.model']),
    license='Apache License 2.0',
    classifiers=[
        'Development Status :: 5 - Production/Stable',
        'License :: OSI Approved :: Apache Software License',
        'Operating System :: Microsoft :: Windows :: Windows 7',
        'Operating System :: Microsoft :: Windows :: Windows 10',
        'Operating System :: POSIX :: Linux',
        'Operating System :: MacOS :: MacOS X',
        'Programming Language :: Python',
        'Programming Language :: Python :: 3',
        'Programming Language :: Python :: 3.6',
        'Programming Language :: Python :: 3.7',
        'Topic :: Games/Entertainment',
        'Topic :: Games/Entertainment :: Role-Playing',
    ],
    python_requires='~=3.7.0',
)
