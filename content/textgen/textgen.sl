#!/bin/bash
#SBATCH --nodes=1
#SBATCH --ntasks=1
#SBATCH --mem-per-cpu=2gb
#SBATCH --gres=gpu:2
#SBATCH --time=04:00:00
#SBATCH --output=<label>.out

cd <wd>

source textgen_venv/bin/activate
source /usr/usc/cuda/10.1/setup.sh
source /usr/usc/cuDNN/v7.6.5-cuda10.1/setup.sh 
 
#nvcc --version
#which python3

srun -N1 -n1 python3 textgen.py <label>

