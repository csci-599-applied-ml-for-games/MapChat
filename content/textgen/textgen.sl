#!/bin/bash
#SBATCH --mail-type=ALL         
#SBATCH --mail-user=sportega@usc.edu	
#SBATCH --nodes=2
#SBATCH --mem=2gb
#SBATCH --gres=gpu:2
#SBATCH --time=00:30:00
#SBATCH --output=<label>.out

cd <wd>

source textgen_venv/bin/activate
source /usr/usc/cuda/10.1/setup.sh
source /usr/usc/cuDNN/v7.6.5-cuda10.1/setup.sh 
 
#nvcc --version
#which python3

srun python3 textgen.py <label>
