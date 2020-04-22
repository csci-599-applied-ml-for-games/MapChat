#!/bin/bash

# number of chunks to split file into
n=6


# split reviews.json into smaller chunks
split -d -a 1 -n $n --additional-suffix=".json" review.json review_ --verbose

# the last and first part of some of the files will be overlaping
# must stich them back together
x=($( seq 0 $((n-2)) )) 
for i in ${x[*]};
do
	# retrieve first line of next file
	last_part=$(cat review_$((i+1)).json | head -n 1)
	sed -i '1d' review_$((i+1)).json 

	# append to the end of current file
	echo $last_part >> review_$i.json
done