# http://joaomdmoura.com/articles/learn-elixir-with-a-rubyist-episode-vii

defmodule Sum do
  def sum_number(x) do
    receive do
      {:sum, n} ->
        IO.inspect(x + n)
        sum_number(x)
    end
  end
end

sum_proc = spawn(Sum, :sum_number, [2])

send(sum_proc, {:sum, 10})
send(sum_proc, {:sum, 6})
send(sum_proc, {:sum, 2})

Process.alive?(sum_proc)
