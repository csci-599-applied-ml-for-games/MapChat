from parse_reviews import readJson
from nltk.tokenize import sent_tokenize
from os import chdir

# currently in src, cd into parent dir (content)
chdir("../")

# label="grocery"
# tags=["food", "fruit", "vegetable", "deli"]

# label="gym"
# tags=["gym", "fitness", "train", "weight", "cardio"]


# label="coffee"
# tags=["coffee", "latte", "creamer", "sugar", "caffeine"]


# label="bank"
# tags=["bank", "money", "service", "account", "saving", "checking", "loan"]


# label connects program to parsed data
label=""
# only save sentences that contain tags
tags=[""]

if label == "":
	print("User needs to specify label")
	exit(1)

# get all reviews with this as the least rating 
rate = 3.0

textgen_dir = "textgen/%s" % (label)
if not os.path.exists(textgen_dir):
	os.mkdir(textgen_dir)

dat = readJson("yelp/parsed/rev/%s.json" % (label))
f = open("%s/%s_train.txt" % (textgen_dir, label), "w")

for obj in dat:	

	# happy reviews
	if obj['rate'] >= rate:
	# if True:	
		# split up review by sentence
		for sentence in sent_tokenize(obj['text']):
			# check number of words in sentence  
			len_sent = len(sentence.split())
			if len_sent >= 4 and len_sent <= 20:
				# done = False
				# only write to file if a tag is present
				sentence = sentence.lower()
				for tag in tags:
					if tag in sentence:
						# txt = "\n".join(sent_tokenize(obj['text']))
						# print(txt, file=f)
						# done = True
						print(sentence.capitalize(), file=f)
						break