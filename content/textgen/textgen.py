import os
import sys
import json
from datetime import datetime
from textgenrnn import textgenrnn



def parse_args():
	if len(sys.argv) <= 1:
		print("need model name (label)")
		exit(1)
	model_name = sys.argv[1]
	file_name = "%s_train.txt" % (model_name)
	if not os.path.exists("%s/%s" % (model_name,file_name)):
		print("enter model name as arg")
		exit(1)
	os.chdir(model_name)
	print("model name: %s" % (model_name))
	return model_name, file_name
'''
def set_tconfig():
	rank=os.environ["SLURM_NODEID"]
	
	nodes = []
	workers = []

	for i in range(2, len(sys.argv)-1, 2):
		node = sys.argv[i]
		nodes.append(node)

		ip = sys.argv[i+1]
		workers.append("%s:5555" % (ip))

	# create config dict
	config={'cluster': 
				{'worker': workers},
			''
			'task': 
				{'type': 'worker', 'index': rank}}

	print(config)

	# set env variable
	os.environ["TF_CONFIG"] = json.dumps(config)
'''


model_name, file_name = parse_args()
print("\n\nstarting training\n\n")


#def train_model(model_name, file_name):
if True:
	model_cfg = {
		'word_level': False,   # set to True if want to train a word-level model (requires more data and smaller max_length)
		'rnn_size': 128,   # number of LSTM cells of each layer (128/256 recommended)
		'rnn_layers': 3,   # number of LSTM layers (>=2 recommended)
		'rnn_bidirectional': False,   # consider text both forwards and backward, can give a training boost
		'max_length': 30,   # number of tokens to consider before predicting the next (20-40 for characters, 5-10 for words recommended)
		'max_words': 10000,   # maximum number of words to model; the rest will be ignored (word-level model only)
	}
	train_cfg = {
		'line_delimited': True,   # set to True if each text has its own line in the source file
		'num_epochs': 20,   # set higher to train the model for longer
		'gen_epochs': 10,   # generates sample text from model after given number of epochs
		'train_size': 0.8,   # proportion of input data to train on: setting < 1.0 limits model from learning perfectly
		'dropout': 0.0,   # ignore a random proportion of source tokens each epoch, allowing model to generalize better
		'validation': False,   # If train__size < 1.0, test on holdout dataset; will make overall training slower
		'is_csv': False   # set to True if file is a CSV exported from Excel/BigQuery/pandas
	}

	textgen = textgenrnn(name=model_name)
	train_function = textgen.train_from_file if train_cfg['line_delimited'] else textgen.train_from_largetext_file
	train_function(
		file_path=file_name,
		new_model=True,
		num_epochs=train_cfg['num_epochs'],
		gen_epochs=train_cfg['gen_epochs'],
		batch_size=1024,
		train_size=train_cfg['train_size'],
		dropout=train_cfg['dropout'],
		validation=train_cfg['validation'],
		is_csv=train_cfg['is_csv'],
		rnn_layers=model_cfg['rnn_layers'],
		rnn_size=model_cfg['rnn_size'],
		rnn_bidirectional=model_cfg['rnn_bidirectional'],
		max_length=model_cfg['max_length'],
		dim_embeddings=100,
		word_level=model_cfg['word_level'])

	# Generate and save content
	temperature = [1.0, 0.5, 0.2, 0.2]   
	prefix = None   # if you want each generated text to start with a given seed text

	if train_cfg['line_delimited']:
		n = 1000
		max_gen_length = 60 if model_cfg['word_level'] else 300
	else:
		n = 1
		max_gen_length = 2000 if model_cfg['word_level'] else 10000
	  
	timestring = datetime.now().strftime('%Y%m%d_%H%M%S')
	gen_file = '{}_gentext_{}.txt'.format(model_name, timestring)
	textgen.generate_to_file(gen_file,
							 temperature=temperature,
							 prefix=prefix, 
							 n=n,
							 max_gen_length=max_gen_length)




# driver

