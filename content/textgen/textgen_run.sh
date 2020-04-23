#!/bin/bash

labels=()
#labels=("coffee" "gym" "bank")

if [ ${#labels[*]} -eq 0 ]; do
	echo "Need to add labels of models you would like to train"
	exit 1
done

for model in ${labels[*]};
do
	echo $model
	cp textgen.sl $model/$model.sl
	cd $model
	sed -i "s|<label>|"$model"|g" $model.sl 
	sed -i "s|<wd>|"$textgen_dir"|g" $model.sl	
	
	sbatch $model.sl
	sleep 1
	cd ..
done