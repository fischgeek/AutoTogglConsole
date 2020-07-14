# AutoTogglConsole

A console version of AutoToggl. Automatically switch your Toggl timers based on the current active window.

## Getting started

If you already have a Toggl account, continue to Setting up your projects. Otherwise, visit [Toggl.com](https://toggl.com/signup) and get yourself an account (there's a free version).

## Setting up your projects

AutoTogglConsole uses your Workspace and Project ID's from your Toggl account. For now, the configuration setup is quite manual. Check back for an update; I plan on making this much easier.

### Before you begin

1. Open the **AutoToggleConsole.exe.config** file in your favorite editor

1. Log into Toggl in your favorite browser

### Set your API Token

1. Head over to [your Profile page](https://toggl.com/app/profile) and retrieve your API Token towards the bottom

### Set your Workspace ID

1. Open your [Toggl Projects page](https://toggl.com/app/projects/)

1. Identify the Workspace ID by inspecting the URL

   - It should look something like this `.../projects/WORKSPACE_ID/list`

1. Update the value in the **AutoToggleConsole.exe.config** file to your Workspace ID
   - `<add key="WorkspaceID" value="YOUR_WORKSPACE_ID" />`

### Setup a Project

1. Open your [Toggl Projects page](https://toggl.com/app/projects/)

1. Click a project you want to track (or create a new one)

1. Identify the Project ID by inspecting the URL

   - It should look like this `.../WORKSPACE_ID/projects/PROJECT_ID/...`

1. Add a new key/value pair below the example in the **AutoToggleConsole.exe.config** file
   - Replace the **key** with the actual Project ID you found
   - Replace the **value** with a comma separated list of keywords that appear in a window's title you consider to be apart of the project
   - You should end up with something like the below
     - `<add key="12345" value="notepad,untitled - notepad,notepad++" />`

### Other Settings to consider

1. **NeutralWindowRegex** This is a regular expression of window title keywords that will neither start nor stop a timer. Edit the **value** as you see fit.

1. **delay** This is the amount of time in milliseconds the application waits before it looks again at the current window. Edit the **value** as you see fit.

Once you have completed editing the configuration file, save and close it and run the main application (AutoTogglConsole.exe) and try it out.

Please feel free to open an issue or [email me](mailto:fischgeek@gmail.com) with any questions or issues you may have. I will try to help the best I can.

Again, I apologize for the terrible setup experience. In the future I plan to make this easier and through the actual UI. In addition, I plan on finding the motivation I misplaced to get my primary idea [AutoToggl](https://) up and running. If you've seen or heard from it, please drop me a line.
