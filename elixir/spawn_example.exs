# http://joaomdmoura.com/articles/learn-elixir-with-a-rubyist-episode-v

defmodule SpawnExample do
  def start do
    IO.puts "=== Starting Application"

    spawn_proc(10)
  end

  def proc_func do
    IO.puts "* Started process"
    :timer.sleep(1000)
    IO.puts "* Finished process"
  end

  defp spawn_proc(0) do
    IO.puts "=== Finishing Application"
  end

  defp spawn_proc(number_of_procs) do
    spawn(__MODULE__, :proc_func, [])

    procs_missing = number_of_procs - 1
    spawn_proc(procs_missing)
  end
end

SpawnExample.start
:timer.sleep(2000)
