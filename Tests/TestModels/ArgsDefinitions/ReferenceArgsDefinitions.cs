using Models.ArgsDefinitions;

namespace TestModels.ArgsDefinitions
{
    public static class ReferenceArgsDefinition
    {
        public static ArgsDefinition ArgsDefinition { get; set; } = new ArgsDefinition()
        {
            DefaultArgs = "help",
            Methods = new Method[]
            {
                new Method()
                {
                    Title = "authorize",
                    Order = 10,
                    Collections = new string[0],
                    Parameters = new string[] {"username", "password"}
                },
                new Method()
                {
                    Title = "download",
                    Order = 40,
                    Collections = new string[] {"productimages", "accountproductimages", "screenshots", "productfiles"},
                    Parameters = new string[] {"id", "os", "lang"}
                },
                new Method()
                {
                    Title = "prepare",
                    Order = 30,
                    Collections = new string[] {"productimages", "accountproductimages", "screenshots", "productfiles"},
                    Parameters = new string[] {"id", "os", "lang"}
                },
                new Method()
                {
                    Title = "update",
                    Order = 20,
                    Collections = new string[]
                    {
                        "products", "gameproductdata", "accountproducts", "apiproducts", "gamedetails", "updated",
                        "wishlisted", "screenshots"
                    },
                    Parameters = new string[] {"id"}
                }
            },
            MethodsSets = new MethodsSet[]
            {
                new MethodsSet()
                {
                    Title = "sync",
                    Methods = new string[] {"update", "prepare", "download"}
                }
            },
            Collections = new Collection[]
            {
                new Collection() {Title = "products"},
                new Collection() {Title = "gameproductdata"},
                new Collection() {Title = "accountproducts"},
                new Collection() {Title = "apiproducts"},
                new Collection() {Title = "gamedetails"},
                new Collection() {Title = "updated"},
                new Collection() {Title = "wishlisted"},
                new Collection() {Title = "screenshots"},
                new Collection() {Title = "accountproductimages"},
                new Collection() {Title = "productimages"},
                new Collection() {Title = "productfiles"}
            },
            Parameters = new Parameter[]
            {
                new Parameter() {Title = "username"},
                new Parameter() {Title = "password"},
                new Parameter() {Title = "id"},
                new Parameter() {Title = "os", Values = new string[] {"windows", "osx", "linux"}},
                new Parameter() {Title = "lang", Values = new string[] {"en"}}
            },
            Dependencies = new Dependency[]
            {
                new Dependency()
                {
                    Method = "update",
                    Collections = new string[]
                        {"accountproducts", "apiproducts", "gamedetails", "updated", "wishlisted"},
                    Requires = new Dependency[]
                    {
                        new Dependency()
                        {
                            Method = "authorize",
                            Collections = new string[0]
                        }
                    }
                },
                new Dependency()
                {
                    Method = "download",
                    Collections = new string[] {"productfiles"},
                    Requires = new Dependency[]
                    {
                        new Dependency()
                        {
                            Method = "authorize",
                            Collections = new string[0]
                        },
                        new Dependency()
                        {
                            Method = "prepare",
                            Collections = new string[] {"productfiles"}
                        },
                        new Dependency()
                        {
                            Method = "update",
                            Collections = new string[] {"gamedetails"}
                        }
                    }
                }
            }
        };
    }
}