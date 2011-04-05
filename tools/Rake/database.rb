require 'rake'
require 'rake/tasklib'

module Rake

class DatabaseTask < TaskLib
	attr_accessor :database_names, :tasks, :import, :export

	def initialize
		@tasks = Hash.new
		@import = Hash.new
		@export = Hash.new
		
		yield self if block_given?
		
		define
	end 
	
	# Create the tasks defined by this task lib.
	def define
		database_names.call().collect do |database_name|
			namespace database_name do
				tasks[:names].collect do |task_name|
					desc "#{task_name.to_s.capitalize} the #{database_name} database"
					task task_name => tasks[:dependencies] do
						tasks[:action].call(task_name, database_name)
					end
				end
				
				add_tasks_for(:import, import, database_name)
				add_tasks_for(:export, export, database_name)
			end
		end
		
		self
	end
	
	def add_tasks_for(ns, hash = {}, database_name = '')
		no_dependencies = lambda { |x| [] }
	
		hash.collect do |hash_data|
			name = hash_data[0]
			data = hash_data[1]
			
			namespace ns do
				desc data[:description] || "#{ns.to_s.capitalize}s #{name.to_s.gsub(/_/, ' ')}"
				task name => (data[:dependencies] || no_dependencies).call(database_name) do
					data[:action].call(database_name)
				end
			end
		end
	end	
end
end