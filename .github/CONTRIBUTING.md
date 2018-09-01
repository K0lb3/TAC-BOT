# Guidelines for Contributing

This guideline is heavily based on [The GLHF Pledge](http://www.anykey.org/take-the-pledge/) and [CONTRIBUTING.rst](https://github.com/sigmavirus24/github3.py/blob/develop/CONTRIBUTING.rst) of github3.py project.

1. Be a good, responsible, and mature sport
1. Remember that people online and in virtual spaces are still real people.
1. Know that my actions and words can have real impact.
1. Regardless of how big or small is your change, add your name to the [AUTHORS.md](./AUTHORS.md).
1. Test your code before you push, no matter how big or small your change is.
1. Rebase your fork/branch before you merge. If you don't know what this is or how to do this, ask for help.

## How to set up your local development environment

This instructions describe for the people who has never set up a Python development environment and provides what the Python development community considers as the best practice. Depending on your prior knowledge, you may skip some steps here and there. Depending on your preference, you may choose not to follow the steps. However, keep in mind that `TAC_bot_db` is set up to be developed using the below instructions.

1. [Download](https://www.python.org/downloads/) and install the latest version of Python 3.
1. After the installation, verify you have `pip` installed and available from your path.
    * `$ pip --version` should return the version number and the location of the executable.
1. Update the `pip` to the latest version: `$ pip install pip --upgrade`
1. Install [`pipenv`](https://docs.pipenv.org/) via `pip`: `$ pip install pipenv`
1. Using `pipenv`, create an environment for `TAC_bot_db`: `$ pipenv --three`
1. Install all dependencies using `pipenv`: `$ pipenv install`
1. Activate the environment created by `pipenv`: `$ pipenv shell`
1. Set the Discord bot token as the environment variable
    * On Unix-like environment: `$ export DISCORD_BOT_TOKEN='MyDiscordBot'`
    * On Windows environment: `C:\TAC_bot_db> set DISCORD_BOT_TOKEN='MyDiscordBot'`
1. Run the bot: `$ python Discord_Bot_V2/Bot.py`
