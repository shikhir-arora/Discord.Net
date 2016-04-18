//Since we have setup our CommandChar to be '~', we will run this command by typing ~greet
_client.GetService<CommandService>().CreateCommand("greet") //create command greet
        .Alias(new string[] { "gr", "hi" }) //add 2 aliases, so it can be run with ~gr and ~hi
        .Description("Greets a person.") //add description, it will be shown when ~help is used
        .Parameter("GreetedPerson", ParameterType.Required) //as an argument, we have a person we want to greet
        .Do(async e =>
        {
            await e.Channel.SendMessage($"{e.User.Name} greets {e.GetArg("GreetedPerson")}");
            //sends a message to channel with the given text
        });