using System;
using System.Linq;
using DryIoc;
using Git = LibGit2Sharp;
using SimplyAOP.IoCExample.Services;

namespace SimplyAOP.IoCExample
{
    static class Program
    {
        static void Main(string[] args) {
            using (var container = new Container()) {
                Setup(container);

                var eventRepo = container.Resolve<IEventRepository>();

                var statService = container.Resolve<StatisticsService>();

                Console.WriteLine("Processing simple string[]");
                Console.WriteLine(statService.Process(new[] {
                    "Hello World",
                    "This is a simple Test",
                    "Hello World",
                    "More strings!"
                }));

                var gitRepo = container.Resolve<Git.Repository>();
                var commitMessages = gitRepo.Commits
                    .Select(c => c.MessageShort);

                Console.WriteLine("Processing git commit messages of this repo.");
                Console.WriteLine(statService.Process(commitMessages));

                Console.WriteLine("Recent events:");
                foreach (var aEvent in eventRepo.Query(TimeSpan.FromHours(1))) {
                    Console.WriteLine(aEvent);
                }
            }
        }

        private static void Setup(Container container) {
            // Event repo and stats service
            container.Register<IEventRepository, InMemoryEventRepository>(Reuse.Singleton);
            container.Register<StatisticsService>();
            // Git
            container.Register(Made.Of(() => new Git.Repository(Arg.Of<string>("repo"))), Reuse.Singleton);
            container.UseInstance("../../../../", serviceKey: "repo");

            // AOP
            container.Register<MethodWatchAdvice>();
            container.Register<EventAdvice>();
            container.Register<AspectConfiguration>(Reuse.Singleton);
            container.RegisterInitializer<AspectConfiguration>((config, ctx) => {
                config.AddAspect(ctx.Resolve<MethodWatchAdvice>());
                config.AddAspect(ctx.Resolve<EventAdvice>());
            });
        }
    }
}
