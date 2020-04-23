#!/bin/bash
wd=""
labels=("")
#labels=("coffee" "gym" "bank")

if wd == "":
	print("Need to add MapChat local path")
	exit(1)

textgen_dir=$wd/content/textgen
cd $textgen_dir
for model in ${labels[*]};
do
	echo $model
	cp textgen.sl $model/$model.sl
	cd $model
	sed -i "s|<label>|"$model"|g" $model.sl 
	sed -i "s|<wd>|"$textgen_dir"|g" $model.sl	
	
	sbatch $model.sl
	sleep 1

	cd $textgen_dir
done


