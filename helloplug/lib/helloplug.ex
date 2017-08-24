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
  #require EEx
  #EEx.function_from_file :defp, :template_show_user, "templates/show_user.eex", [:user_id]
  def route("GET", ["users", user_id], conn) do
    case Helloplug.Repo.get(User, user_id) do
      nil ->
        conn |> Plug.Conn.send_response(404, "User with that ID not found, sorry!")
      user ->
        #page_contents = EEx.eval_file("templates/show_user.eex", [user_id: user_id])
        # Updated view to use template
        #page_contents = template_show_user(user_id)
        # Updated view to use user object
        page_contents = EEx.eval_file("templates/show_user.eex", [user: user])
        conn |> Plug.Conn.put_resp_content_type("text/html") |> Plug.Conn.send_resp(200, page_contents)
    end
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

    timestamps()
  end
end

# A test!
# It breaks the program though so I'm guessing it's not supposed to go here. :)
#defmodule HelloTest do
#  use ExUnit.Case, async: true
#  use Plug.Test

#  @website_router_opts WebsiteRouter.init([])
#  test "returns a user" do
#    conn = conn(:get, "/users/1")
#    conn = WebsiteRouter.call(conn, @website_router_opts)

#    assert conn.state == :sent
#    assert conn.status == 200
#    assert String.match?(conn.resp_body, ~r/Fluffums/)
#  end
#end
