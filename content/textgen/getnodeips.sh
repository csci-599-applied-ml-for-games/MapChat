#!/bin/bash




nodes=$SLURM_JOB_NODELIST
nodes=${nodes:4}
nodes=${nodes::-1}

nodes=($(echo $nodes | sed 's/[,-]/ /g'))

i=0
for node in ${nodes[*]}; do
	#echo $node
	ip=($(ssh hpc$node hostname -I))
	ip=${ip[0]}
	echo hpc$node $ip
done
