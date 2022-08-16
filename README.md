# Gatekeeper
Discord bot for verification on MFF non-official discord server.

## Features
The bot main feature - **verification** is inherently tied to SIS, so no versitality there.
The other main thing - **reaction channel permissions** can be used to give individual view permissions to selected channel when user reacts to selected message.

## Future plans
* add resource file for all the strings
* much better error handling
* even more logging
* slowly adding functionality that will be needed in the DC

## Getting started
### Requirements
* `.NET 6.0 or greater` which can be downloaded [here](https://dotnet.microsoft.com/en-us/download)
* DSharpPlus library should be downloaded automatically

### Using the bot
* download and build the project
* To use the bot you will need to fill out the `stub_configuration.ini`, then save it as `configuration.ini` or however you like but same as the start argument that you will pass to the executable
  * only 3 hashing function are supported so far - sha256, sha512 and md5 (all lowercase)
  * you need to pass the configuration file name to the executable as an argument!
* after that you can run the program and you are off!
* you can use `~help` to get all aviable commands
  * `react_to_perms` is a interation loop
  * `verify <ukco>` and `code <code>` go togther to verify user

## Programmer documentation
I will focus on a high-level overview rather than individual function. If you want documentation for individual functions there are XML docs.

### Verification
* problem to solve 
  * telling if user is from mff and also uniqueness of users
* verification has X steps
  * get the email from SIS
  * send random code to email
  * (code is then stored and waiting for check)
  * user inputs code - the code is checked
  * hashed ukco is writen to the hash file
  * (user gets predefined role) // not implemented due to it being hardcoded to role
  
### Reaction channel permission
* problem to solve 
  * get inidividiual view permission to channel based on reaction on certain message
* all reactions that bot seas fire the MessageReactionAdded event
  * we use this to filter the corect message/reaction to get the permission
* harder part - setting the message and reaction
  * since there can be infinite reaction -> channel combination I settled on interation loop - bot with user
    * I might change this in the future - its quite cumbersome
  * user initiates the command via `react_to_perms`
    * then the interaction framework is used to make a "dialoge" between bot and user
    * user sets all the values

### Handlers
* there are several handlers each having a specific task and an interface to hide behind
* for example - SisHandler relies on HttpClient that is injected into it
  * the SisHandler is also hidden behind the interface that is used by the command module (and again that instance is injected into it)

### Commands
* commands are structured to the moduels as per DShaprGuidelies
* there are 2 modules
  * one for **verification** and one for **reaction channel permission**

### Dependency Injection
* program heavily relies on dependency injection c# framework
* the Dsharp library encourages this wia services property in CommandsNextConfiguration
* the handlers are created as singletons so they can be reused in entire assembly
* this way the data/handlers are gotten into the command modules quiet easily through contructor and are stored as private

### Performace
* performance is not a focus of this codebase, but it makes some nice use of async/await programming
