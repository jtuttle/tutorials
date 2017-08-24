# See: https://codewords.recurse.com/issues/five/building-a-web-framework-from-scratch-in-elixir

defmodule Router do
  defmacro __using__(_opts) do
    quote do
      def init(options) do
        options
      end
      
      def call(conn, _opts) do
        route(conn.method, conn.path_info, conn)
      end
    end
  end
end

# iex -S mix
# {:ok, _} = Plug.Adapters.Cowboy.http Helloplug, []
defmodule Helloplug do
  use Router

  # We moved these two methods to the Router macro.
  #def init(default_opts) do
  #  IO.puts "Starting Helloplug..."
  #  default_opts
  #end

  #def call(conn, _opts) do
  #  IO.puts "saying hello!"
  #  route(conn.method, conn.path_info, conn)
  #end

  def route("GET", ["hello"], conn) do
    conn |> Plug.Conn.send_resp(200, "Hello, world!")
  end
  
  def route(_method, _path, conn) do
    conn |> Plug.Conn.send_resp(404, "Couldn't find that page, sorry!")
  end
end

# iex -S mix
# {:ok, _} = Plug.Adapters.Cowboy.http UserRouter, []
defmodule UserRouter do
  use Router

  # These two lines pre-compile the template so that we don't have to eval_file on every request.
  require EEx
  EEx.function_from_file :defp, :template_show_user, "templates/show_user.eex", [:user_id]
  def route("GET", ["users", user_id], conn) do
    #page_contents = EEx.eval_file("templates/show_user.eex", [user_id: user_id])
    page_contents = template_show_user(user_id)
    
    conn |> Plug.Conn.put_resp_content_type("text/html") |> Plug.Conn.send_resp(200, page_contents)
  end

  def route("POST", ["users"], conn) do
    # create a new user
  end

  def route(_method, _path, conn) do
    conn |> Plug.Conn.send_resp(404, "Couldn't find that user page, sorry!")
  end
end

# iex -S mix
# {:ok, _} = Plug.Adapters.Cowboy.http WebsiteRouter, []
defmodule WebsiteRouter do
  use Router

  @user_router_options UserRouter.init([])
  def route("GET", ["users" | path], conn) do
    IO.puts "routing"
    UserRouter.call(conn, @user_router_options)
  end

  def route(_method, _path, conn) do
    conn |> Plug.Conn.send_resp(404, "Couldn't find that page, sorry!")
  end
end

# Module to hold functions that query our database.
defmodule Helloplug.Repo do
  use Ecto.Repo,
    otp_app: :helloplug,
    adapter: Sqlite.Ecto
end

# Module to represent our User model
# Create migration w/ `mix ecto.gen.migration create_users`
defmodule User do
  use Ecto.Model

  schema "users" do
    field :first_name, :string
    field :last_name, :string
  end
end

# TODO: got stuck here, because the command to create a migration is failing with the error:
# ** (ArgumentError) configuration for Helloplug.Repo not specified in :helloplug environment
# even though (I think) this is what I'm doing in config/config.exs
