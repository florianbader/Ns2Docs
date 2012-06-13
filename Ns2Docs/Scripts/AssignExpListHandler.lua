local AssignExpListHandler = {}

function AssignExpListHandler.Capture(expresions)
	if type(expresions) ~= "string" then
		return expresions
	end
	
	local match = expList.pattern:match(expresions)
	
	local tbl = {}
	local i = 1
	while match[i] ~= nil do
		table.insert(tbl, expresions:sub(match[i], match[i+2]-1))
		i = i + 3
	end
	return tbl
end

return AssignExpListHandler