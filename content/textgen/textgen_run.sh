#!/bin/bash
wd="/home/rcf-40/sportega/ml4g/textgen"
#models=("coffee" "gym")
models=("gym")

cd $wd

for model in ${models[*]};
do
	#mkdir -p $model
	cp textgen.sl $model/$model.sl
	cd $model
	
    sed -i "s|<label>|"$model"|g" $model.sl 
	sed -i "s|<wd>|"$wd"|g" $model.sl	
	
	sbatch $model.sl 
	cd $wd
done


