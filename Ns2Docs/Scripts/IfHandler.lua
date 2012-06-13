local Library = Ns2Docs.Spark.Library

local IfHandler = {}

function IfHandler.Capture(expression, block, ...)
	local tbl = {
		expression=expression, 
		block=block,
		Create = IfHandler.Create
	}
	
	local t = {}
	for i=1, #arg do
		if type(arg[i]) == "string" then
			table.insert(t, {expression=arg[i], block=arg[i+1], Create=IfHandler.Create})
			i = i+1
		else
			table.insert(t, {block=arg[i], Create=IfHandler.Create})
		end
	end

	return {
		Create = function(result, source, info, extraData)
			tbl.Create(result, source, tbl, extraData)
			for index, value in ipairs(t) do
				tbl.Create(result, source, value, table.copy(extraData))
			end
		end
	}

end

function IfHandler.Create(result, source, info, extraData)
	extraData = extraData or {}
	
	if info.expression == "Server" then
		extraData.Library = Library.Server
	else
		extraData.Library = Library.Client
	end
	
	if info.block.content then
		for index, value in pairs(info.block.content) do
			if value.Create then
				value.Create(result, source, value, extraData)
			end
		end
	end
end

return IfHandler