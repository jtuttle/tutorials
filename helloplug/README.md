# Source

https://codewords.recurse.com/issues/five/building-a-web-framework-from-scratch-in-elixir

# Requirements

- `elixir`

# Execution

```
mix deps.get
mix ecto.migrate
iex -S mix
{:ok, _} = Plug.Adapters.Cowboy.http UserRouter, []
```
Then visit `localhost:4000/users/123` in browser.
