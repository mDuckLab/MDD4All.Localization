Switches the language of a running application, and makes the switch reach everything.

The usual way is CultureInfo.CurrentUICulture. That value travels with the execution
context, and there are hosts where it never arrives - a Blazor renderer keeps the culture
it started with, whatever is assigned afterwards. Measured, not assumed.

So the culture is handed over rather than read. An ILanguageSetter holds the current one
and announces a change; a string localizer factory placed in front of the built-in one
asks that setter instead of the ambient culture. Because StringLocalizer<T> gets its
texts from whichever factory is registered, this reaches third-party components too,
without touching a line of them.

Targets netstandard2.0.
