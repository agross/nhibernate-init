require 'rake/clean'
require 'configatron'
Dir.glob(File.join(File.dirname(__FILE__), 'tools/Rake/*.rb')).each do |f|
	require f
end

task :default => [:clobber, 'compile:app', 'tests:run']

desc 'Runs a quick build just compiling the libs that are not up to date'

namespace :env do
	Rake::EnvTask.new do |env|
		env.configure_from = 'properties.yml'
		env.configure_environments_with = lambda do |env_name|
			configure_env_for env_name
		end
	end
	
	def configure_env_for(env_key)
		env_key = env_key || 'development'

		puts "Loading settings for the '#{env_key}' environment"
		
		yaml = Configuration.load_yaml 'properties.yml', :hash => env_key, :inherit => :default_to, :override_with => :local_properties
		configatron.configure_from_hash yaml
		
		if configatron.deployment.exists?(:instance_name) and configatron.deployment.instance_name
			configatron.deployment.root = "#{configatron.deployment.root}_#{configatron.deployment.instance_name}" 
			instanceName = "-#{configatron.deployment.instance_name}"
		end
		
		configatron.deployment.backups = File.join(configatron.deployment.root.to_s, "Backups")
		
		CLEAN.clear
		CLEAN.include('teamcity-info.xml')
		CLEAN.include('**/obj'.in(configatron.dir.source))
		CLEAN.include('**/*'.in(configatron.dir.test_results))
				
		CLOBBER.clear
		CLOBBER.include(configatron.dir.build)
		CLOBBER.include(configatron.dir.deploy)
		CLOBBER.include('**/bin'.in(configatron.dir.source))
		
		configatron.protect_all!

		puts configatron.inspect
	end

	# Load the default environment configuration if no environment is passed on the command line.
	Rake::Task['env:development'].invoke \
		if not Rake.application.options.show_tasks and
		   not Rake.application.options.show_prereqs and
		   not Rake.application.top_level_tasks.any? do |t|
			/^env:/.match(t)
		end
end

namespace :compile do
	desc 'Compiles the application'
	task :app do
		FileList.new("#{configatron.dir.source}/**/*.csproj").each do |project|
			MSBuild.compile \
        :clrversion => 'v4.0.30319',
				:project => project,
				:properties => {
					:SolutionDir => configatron.dir.source.to_absolute.chomp('/').concat('/').escape,
					:Configuration => configatron.build.configuration
				}
		end
	end
end

namespace :tests do
	desc 'Runs unit tests'
	task :run => ['compile:app'] do
		FileList.new("#{configatron.dir.source}/**/bin/**/NHibernate-Init.*.dll").each do |assembly|
			Mspec.run \
				:tool => configatron.tools.mspec,
				:reportdirectory => configatron.dir.test_results,
				:assembly => assembly
		end
	end
end