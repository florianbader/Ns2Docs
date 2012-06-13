local DislocatedComment = {}

function DislocatedComment.Capture(offset, comment)
	return 
	{
		__name="Assign", 
		comment=comment, 
		line=line,
		offset=offset,
		Create = DislocatedComment.Create
	}
end



function DislocatedComment.Create(game, source, info, extraData)
	Ns2Docs.Spark.DislocatedComment(game, source, info.comment, extraData.Library, info.offset)
end

return DislocatedComment