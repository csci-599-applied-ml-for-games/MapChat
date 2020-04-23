#!/bin/bash

# connect to uschpc python3.7 library
source /usr/usc/python/3.7.4/setup.sh

# create a python3 virual environment
python3 -m venv textgen_venv

# activate new virtual environment
source textgen_venv/bin/activate

# all packages installed to local virtual env directory
python3 -m pip install tensorflow-gpu
python3 -m pip install tensorflow 
python3 -m pip install textgenrnn
