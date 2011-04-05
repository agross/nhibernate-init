require 'digest/md5'

class WebAppReferences
	def self.create(attributes)
		files = [] << attributes.fetch(:files)
		template = attributes.fetch(:template)
		out_file = attributes.fetch(:out_file)
		
		lines = []
		
		files \
			.flatten \
			.sort_by { |file| file.pathmap('%X') } \
			.each do |file|
			hash = Digest::MD5.hexdigest(File.read(file))
			
			line = template.call "#{file.name}?#{hash}"
			lines << line
		end
		
		File.open(out_file, 'w' ) do |output|
			lines.each do |line|
				output.puts line
			end
		end
		
		puts "Created file #{out_file}"
	end
end