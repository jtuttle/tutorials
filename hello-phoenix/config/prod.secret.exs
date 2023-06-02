use Mix.Config

# In this file, we keep production configuration that
# you'll likely want to automate and keep away from
# your version control system.
#
# You should document the content of this
# file or create a script for recreating it, since it's
# kept out of version control and might be hard to recover
# or recreate for your teammates (or yourself later on).
config :hello_phoenix_heroku, HelloPhoenixHeroku.Endpoint,
  secret_key_base: "CSJCMOzkq8ahuLs0h0SvAE+JlypnrVXzjNusuNQMSqxV7T3tepNiYCaGY+DtFAbk"

# Configure your database
config :hello_phoenix_heroku, HelloPhoenixHeroku.Repo,
  adapter: Ecto.Adapters.Postgres,
  username: System.get_env("DATABASE_USERNAME"),
  password: System.get_env("DATABASE_PASSWORD"),
  database: "hello_phoenix_heroku_prod",
  pool_size: 15
