# DependencyInjection.ServiceCollectionExtensions

There are two features I would like to see added to `Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions`, both that would be solved with one two extension method.

The first is to be able to specify the lifetime when adding services to the collection. As a library maintainer, people have asked me to add a one line "services.AddLibrary()" extension method, but currently the easiest way to implement Transient, Singleton, or Scoped service lifetimes is to copy and paste registrations then rename. The service collection has a way to do this, but it is not exposed for external usage.

The other feature I'd like to use is to be able to register classes that implent more than one interface. These limitations have been highlighted multiple times before, e.g. https://github.com/aspnet/DependencyInjection/issues/360.

Both of these limitations can be solved, so why not make it accessible to everyone else?