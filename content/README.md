# Content Generation

## Python Environment Install 

### Create Python 3 virtual environment:
```sh
 python3 -m venv venv
```
### Activate virtual environment
```sh
 source venv/bin/activate
```
### Install dependencies 
```sh
 pip -r venv_req.txt
```


## Data

### Source
We used Yelp's [NLP dataset](https://www.yelp.com/dataset)

### Setup
In order to use our scripts:

 1. Download Yelp dataset

 2. Put 'review.json' and 'business.json' into 'MapChat/yelp/raw'

 3. If not enough memory on local machine, run 'split_reviews.sh' to split file into smaller chunks to be processed sequentially.

### Parse Reviews
edit src/parse_reviews.py

add local path to repo

add label and list of 'categories' to specifiy which business reviews to parse

run parse_review

edit src/format_reviews.py

add label and list of 'tags' containg words to parse the reviews one more time 

run format review

Check out the training data that was created! 

Keep tweaking the tags to get more specified reviews



## Train 

### python env



### USCHPC env


### run