# http://joaomdmoura.com/articles/learn-elixir-with-a-rubyist-episode-v

defmodule RecursionExample do
  def print_list([first_element | rest_of_list]) do
    IO.puts first_element
    print_list(rest_of_list)
  end

  def print_list([]) do
    IO.puts "Finished list"
  end
end

RecursionExample.print_list([1,2,3,4,5])
