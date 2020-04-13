
from yelp_reviews import *
from nltk.tokenize import sent_tokenize

# label="grocery"
# tags=["food", "fruit", "vegetable", "deli"]

# label="gym"
# tags=["gym", "fitness", "train", "weight", "cardio"]


# label="coffee"
# tags=["coffee", "latte", "creamer", "sugar", "caffeine"]


# label="bank"
# tags=["bank", "money", "service", "account", "saving", "checking", "loan"]


# label="dorm"
# tags=[""]

label="clothes"
tags=[""]

textgen_dir = "textgen/%s" % (label)
if not os.path.exists(textgen_dir):
	os.mkdir(textgen_dir)

dat = readJson("yelp/parsed/rev/%s.json" % (label))
f = open("%s/%s_train.txt" % (textgen_dir, label), "w")

for obj in dat:	

	# happy reviews
	#if obj['rate'] >= 1.0:
	if True:	
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

				# if done: 

				# 	for 

				# 	break

		# exit(1)

