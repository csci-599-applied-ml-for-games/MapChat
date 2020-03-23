from textgenrnn import textgenrnn

textgen = textgenrnn()
textgen.train_from_file('text.txt', num_epochs=1)

print("-----------------------\n\n")
for i in range(5):
	print("-------------")
	print(i)
	textgen.generate()

# def formatJson(filename):
# 	with open(filename, "r") as file, open("text.txt", "w") as out:	
# 		for line in file:
# 			print(line[44:(len(line)-2)], file=out)

# import numpy
# import sys
# from nltk.tokenize import RegexpTokenizer
# from nltk.corpus import stopwords
# from keras.models import Sequential
# from keras.layers import Dense, Dropout, LSTM
# from keras.utils import np_utils
# from keras.callbacks import ModelCheckpoint

# def tokenize_words(input):
#     # lowercase everything to standardize it
#     input = input.lower()

#     # instantiate the tokenizer
#     tokenizer = RegexpTokenizer(r'\w+')
#     tokens = tokenizer.tokenize(input)

#     # if the created token isn't in the stop words, make it part of "filtered"
#     filtered = filter(lambda token: token not in stopwords.words('english'), tokens)
#     return " ".join(filtered)


# file = open("text.txt").read()
# print(file)
# processed_inputs = tokenize_words(file)
# print(processed_inputs)
