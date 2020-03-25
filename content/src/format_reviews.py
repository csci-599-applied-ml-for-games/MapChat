
from yelp_reviews import *
from nltk.tokenize import sent_tokenize

# label="grocery"
# tags=["food", "fruit", "vegetable", "deli"]

label="gym"
tags=["gym", "fitness", "train", "weight", "cardio"]


label="coffee"
tags=["coffee", "latte", "creamer", "sugar", "caffeine"]


dat = readJson("yelp/parsed/rev/"+label+".json")
f = open("textgenrnn/%s/%s_train.txt"%(label, label), "w")

for obj in dat:	

	# happy reviews
	if obj['rate'] >= 4.0:
		
		# split up review by sentence
		for sentence in sent_tokenize(obj['text']):
			# check number of words in sentence  
			len_sent = len(sentence.split())
			if len_sent >= 4 and len_sent <= 20:

				# only write to file if a tag is present
				sentence = sentence.lower()
				for tag in tags:
					if tag in sentence:
						print(sentence.capitalize(), file=f)
						break

		# exit(1)