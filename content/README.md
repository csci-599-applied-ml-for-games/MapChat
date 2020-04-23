# Content Generation for MapChat

## Python Environment Install

Before you can use any of our python scripts you must first setup your [python virtual environment](https://docs.python.org/3/tutorial/venv.html).

#### Create Python 3 Virtual Environment:
```sh
 # create venv
 python3 -m venv venv
```

#### Activate Virtual Environment
```sh
 # activate venv
 source venv/bin/activate
```

#### Install Packages
All required package names and versions can be found in requirements.txt
```sh
 # install requirements
 pip -r requirements.txt
```

## Data
In order to generate text that we can use to "chat" with our users, we need to use some Machine Learning technique to train on examples of real people, talking about real world things. We decided that the most appropriate dataset to use for this purpose would be Yelp's [NLP dataset](https://www.yelp.com/dataset), which contains over 5GB of real user reviews.

#### Setup Yelp Data
In order to use our scripts you must first:
- Download Yelp dataset
- Put 'review.json' and 'business.json' into 'yelp/raw'
- If not enough memory on local machine, run 'split_reviews.sh' to split file into smaller chunks to be processed sequentially.

## Parse Reviews for Training Text
At this point you have the whole Yelp NLP dataset. Now we have to parse this data for more specific reviews so we can train our models on specific topics. We are essentially going to repeat this process to make a model for each desired topic/label (coffee, gym, grocery, etc).

#### parse_reviews.py
- Open src/parse_reviews.py
- Add MapChat local path in global variable "wd"
- Add "label" and list of "categories" (business categories to filter reviews)
    - All valid Yelp business categories found [here](https://github.com/csci-599-applied-ml-for-games/MapChat/blob/master/content/yelp/parsed/bus/business_categories.txt). 
- Run "python3 parse_review"

#### format_reviews.py
- Open src/format_reviews.py
- Add "label" and list of "tags" (words to filter review sentences)
- Run "python3 format_review"
    - All parsed training data can be found at: textgen/<label>/<label>_train.txt


## Train 
At this point we have our training data ready. Next step is training via some Machine Learning.

#### Textgenrnn
We used [textgenrnn](https://github.com/minimaxir/textgenrnn) package to train on our parsed Yelp data and generate content. This package is ready to use out of the box, while utilizing Tensorflow (Neural Network) backend.

#### CUDA and CudNN
Training texgenrnn without a GPU takes too long. If GPU is available, make sure to download and install CUDA and cudNN. If confused about the compatibility of tensorflow/CUDA/cuDNN versions check this useful [stackoverflow post](https://stackoverflow.com/questions/50622525/which-tensorflow-and-cuda-version-combinations-are-compatible).

#### Training on USCHPC
With computing resources being scarce on my local machine, I decided to do all of our training on [USC's Supercomputer](https://hpcc.usc.edu/gettingstarted/), which has a ton of GPU's and CUDA/cuDNN already installed!

#### textgen_install.sh
- This script will setup your python virtual environment for textgenrnn training on uschpc.
- Run once "./textgen_install.sh"

#### textgen_run.sh
- Open textgen/textgen_run.sh
- Add local path of MapChat into 'wd' 
- Add list of 'labels' to start training
- Run "./textgen_run.sh"
	- A batch job for each label/model will be submitted to the USCHPC job scheduler.
	- Weights from training and generated text can be found at: textgen/<label>/<label>_gentext.txt
