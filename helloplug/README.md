# Source

https://codewords.recurse.com/issues/five/building-a-web-framework-from-scratch-in-elixir

# Requirements

- `elixir`

# Execution

```
iex -S mix
{:ok, _} = Plug.Adapters.Cowboy.http UserRouter, []
```
Then visit `localhost:4000/users/123` in browser.

# Note
I got stuck because the command to create a migration is fails with the error:
```
** (ArgumentError) configuration for Helloplug.Repo not specified in :helloplug environment
```
even though (I think) this is what I'm doing in config/config.exs...