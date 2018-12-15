# Welcome to Puppet

A .NET Core framework for automating Hubitat Elevate and SmartThings.

## How it works

1. An event on one of your devices is triggered. 
    Example: Your pantry door opens, triggering a contact sensor to send **open**.

2. An instance of [this app](https://github.com/CamSoper/Hubitat-CamSoper/tree/master/hubitat-webhooks) is configured to send a webhook to the Puppet Web app.
    Example: The webhook app posts the event containing the **open** status to `http://some.address.on.my.network:5000/api/automation/?automationName=pantry_door`

3. The web app handles the request by passing the event information to the Puppet Executive app, which runs in a separate process that manages task state.

4. The Puppet Executive passes the name of the automation to the `AutomationFactory` to get the appropriate `IAutomation`. The `AutomationFactory` class instantiates the correct implementation of `IAutomation`, in this case and returns it to the Executive.
    Example: The Executive asks `AutomationFactory` for an object to handle the automation named `pantry_door`. It returns an instance of `Puppet.Automation.PantryLightAutomation`.

5. The Executive executes the `Handle()` method on the IAutomation object. The object can manipulate other devices via the `HomeAutomationPlatform` class, of which an instance can be passed in by the Executive.
    Example: The logic in `Puppet.Automation.PantryLightAutomation` should be pretty self-explanatory. :)

## Building

Either build the entire solution with Visual Studio or build the Puppet.Web and Puppet.Executive projects at the command line by going to each of those project directories and running `dotnet build`.

## Setup the Webhooks

Create a new instance of the Webhooks app in your Hubitat for each automation, and assign only the devices that trigger *that specific automation*.

The reason for this is because each time an automation is executed, a cancellation token is sent to all the currently running instances.  So if you have 5 automations in your Puppet system, you will likewise want to have 5 installed instances of the Webhooks Hubitat app.

Plus, it's an efficient way of ensuring that the Hubitat only sends events for devices you want to hear from.

## Setup the Maker API app

It's built-in to the Hubitat. Make sure you've turned it on.

## Running

I've not tried to run the web app in IIS. I just run it with the self-contained web server, Kestrel.

1. Run the web app by switching to the Puppet.Web folder and running `dotnet run --urls=http://*:5000`
    *Note the URLs flag - It's important!*
2. In another command prompt window, switch to the Puppet.Executive folder and run `dotnet run`. It will connect to the web app at localhost:5000.

## Deployment to a Raspberry Pi

It'll run fine on Windows, but I've not done it. I use a Raspberry Pi running Raspbian for my instance.

1. Publish Puppet.Web and Puppet.Executive with the following:
    `dotnet publish -r linux-arm`
    
    This will build a self-contained deployment, including the SDK, so you won't need to install the SDK on the RPi.

2. Using your file copy tool of choice, copy the contents, which you'll find several levels deep in the *bin* directory, to a location on your RPi. I chose `/home/pi/web` and `/home/pi/executive`.

3. SSH to your Raspberry Pi. For each of those applications, give the executables permission to run. For example:
    ```
    chmod 755 /home/pi/web/Puppet.Web
    chmod 755 /home/pi/executive/Puppet.Executive
    ```

4. Test the applications by running them manually. In one SSH session, run the web app by name, like this: `./Puppet.Web --urls=http://*:5000`. In another, run the Executive with `./Puppet.Executive`. **CTRL-C** to end them both.

5. Setup crontab on your Raspberry Pi to run the two apps in the background on startup. Run `crontab -e` and add the following two lines:
    ```
    @reboot /home/pi/web/Puppet.Web --urls=http://*:5000 > /home/pi/web.log
    @reboot cd /home/pi/executive && ./Puppet.Executive > /home/pi/executive.log
    ```

    This will run both of the apps at startup and pipe their output to log files in the home directory.

6. Reboot. `sudo reboot`

## Developing new automations

1. Add an instance of the Webhooks app to your Hubitat. Be sure to include the `automationName` parameter.
2. Add a class to `Puppet.Automation`. Name it whatever you want. I like ending with `Automation` as a convention, but do whatever you like.
    * Make it implement `Puppet.Common.Automation.IAutomation`.
    * Give it a constructor that takes a single `Puppet.Common.Services.HomeAutomationPlatform` if you want it to be able to do stuff to other devices on your Hubitat.
3. Add a case to `Puppet.Executive.AutomationFactory` to return an instance of the class you created.
    * **IMPORTANT** - I don't plan for `AutomationFactory` to be hard-coded forever. One of the enhancements I want to make at some future point is I want it to read the automations dynamically at runtime, and I'll probably pass the name of the class in the `automationName` parameter. For that reason I *strongly* recommend you use the same string for both the name of your `IAutomation` class and your `automationName` parameter.

## Enjoy!

I hope you like my work here. If you're having trouble, that's understandable - I wrote it for *me*, and sometimes that means that I don't think through usability stuff. Create an issue in this repo, or, better yet, send me a PR!

Lastly, please check out my social media presences!

* [Twitter](https://twitter.com/camsoper)
* [Twitch](https://twitch.tv/CamDoesCoolStuff) (I stream programming stuff)
* [My Twitch Archive on YouTube](https://www.youtube.com/playlist?list=PL7390OIw2znaTPK4GGCtRnoJe1scVl5ZT)

\- Cam
