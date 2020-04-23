import sys
import json
import os
import time

# cd into content dir
os.chdir("../")

''' driver code '''

def main():

	# label to store parsed data ('gym', 'coffee', etc)
	label = ""

	# categories of businesses to parse. 
	# all valid business categories can be found at:
	# content/yelp/parsed/bus/business_categories.txt
	categories = [""]


	if label == "":
		print("User needs to provide label and category list")
		exit(1)

	# parse and store reviews from appropriate businesses
	categoryReviews(categories, label)

	# categories ex:

	# label = "coffee"
	# cats = ["Coffee & Tea", "Coffee Roasteries", "Coffeeshops"]

	# label = "grocery"
	# cats = ["Grocery"]

	# label = "gym"
	# cats = ["Gyms"]

	# label = "bank"
	# cats = ["Banks & Credit Unions"]

	# label = "dorm"
	# cats = ["University Housing"]

	# label = "clothes"
	# cats = ["Children's Clothing", "Men's Clothing", "Women's Clothing"]


''' given filename, read every json obj
	return list
'''
def readJson(filename):
	# print("reading " + filename)
	data = []
	with open(filename, "r") as read_file:	
		for line in read_file:
			data.append(json.loads(line))
	# print("finish reading " + filename)
	return data


''' given some list of json objs, 
	 	count every unique business category
	return catergories to file
'''
def getAllCategories(bus_json):
	categories = {}
	for i, obj in enumerate(bus_json):
		obj_cats = obj['categories']
		if isinstance(obj_cats, str):
			obj_cats = obj_cats.split(",")
			for cat in obj_cats:  
				cat = cat.strip()
				if categories.get(cat, 0):
					categories[cat] += 1
				else:
					categories[cat] = [business]
	out = open("yelp/parsed/bus/categories.txt", "w")
	keys = list(categories.keys())
	keys.sort()
	for key in keys:
		print(key,categories[key], file=out)

''' given a list of desired categories,
	 	parse yelp business json for relevant businesses
	return list of business ID's and write to file
'''
def getBusinesses(bus_json, categories, label):
	
	bus_id = []

	# if file doesnt exist, create
	if not os.path.exists("yelp/parsed/bus/"+label+".json"):
		print("first time parsing business ids for label: " + label)

		f = open("yelp/parsed/bus/"+label+".json", "w")
		# for every business
		for obj in bus_json:
			# check if obj 'categories' value is valid
			obj_cats = obj['categories']
			if isinstance(obj_cats, str):	
				# split string incase of multiple categories
				obj_cats = obj_cats.split(",")
				# for every category for this business
				for cat in obj_cats:  
					# check if business is relevant 
					cat = cat.strip()
					if cat in categories:
						line = "{"+'"id":"{}", "name":"{}"'.format(obj['business_id'], obj['name'])+"}"
						print(line, file=f)
						bus_id.append(obj['business_id'])
						break;
		f.close()

	# if does, read and return
	else:

		for obj in readJson("yelp/parsed/bus/"+label+".json"):
			bus_id.append(obj['id'])

	return bus_id

''' given a list of business IDs,
		get all relevant reviews for yelp reviews json
	return reviews to file
'''
def getReviews(rev_json, businesses, label):
	# with open("yelp/small_review.json", "r") as rev_json:	
	f = open("yelp/parsed/rev/"+label+".json", "a+")
	for obj in rev_json:
		# found review for business of interest
		if obj['business_id'] in businesses:

			text = obj['text'].replace("\n", " ")

			new_obj = {"bus_id" : obj['business_id'],
					   "rate"	: obj['stars'],
					   "text"	: text}

			json.dump(new_obj, f)
			print("", file=f)

	f.close()

''' given some categories,
		get every review associated
'''
def categoryReviews(cats, label):
	# get yelp json data
	bus_json = readJson("yelp/raw/business.json")
	print("--------------------")
	print("starting " + label)
	bus = getBusinesses(bus_json, cats, label)
	print("located all business related")

	# remove file if exists
	label_filepath = "yelp/parsed/rev/"+label+".json"
	if os.path.exists(label_filepath):
	    os.remove(label_filepath)

	# for each review file (file too big, split it up into 6 even chunks)
	total_time = 0
	for i in range(6):
		start_time = time.time()
		rev_filename = "review_"+str(i)+".json"

		print("opening %s" % (rev_filename))
		rev_json = readJson("yelp/raw/" + rev_filename)

		print("searching through %s" % (rev_filename))
		revs = getReviews(rev_json, bus, label)
		
		finish_time = (time.time() - start_time)
		total_time += finish_time
		print("finished %s in %s sec" % (rev_filename, finish_time))

	print("total time: %s" % (total_time))

if __name__ == "__main__":
	main()