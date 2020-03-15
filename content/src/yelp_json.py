import sys
import json

''' given filename, read every json obj
	return list
'''
def readJson(filename):
	print("reading " + filename)
	data = []
	with open(filename, "r") as read_file:	
		for line in read_file:
			data.append(json.loads(line))
	
	print("finish reading " + filename)

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
	out = open("../bus/categories.txt", "w")
	keys = list(categories.keys())
	keys.sort()
	for key in keys:
		print(key,categories[key], file=out)

''' given a list of desired categories,
	 	parse yelp business json for relevant businesses
	return list of businesses and write to file
'''
def getBusinesses(bus_json, categories, filename):
	bus_id = []
	f = open("../bus/"+filename, "w")

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
	return bus_id

''' given a list of business IDs,
		get all relevant reviews for yelp reviews json
	return reviews to file
'''
def getReviews(rev_json, businesses, filename):
	# with open("yelp/small_review.json", "r") as rev_json:	
	f = open("../rev/"+filename, "w")
	for obj in rev_json:
		# found review for business of interest
		if obj['business_id'] in businesses:
			text = obj['text'].replace('\n', ' ').replace("\"", "'")
			line = '"bus_id":"{}", "text":"{}"'.format(obj['business_id'], text)
			line = "{"+line+"}"
			print(line, file=f)
	f.close()

''' given some categories,
		get every review associated
'''
def categoryReviews(bus_json, rev_json, cats, filename):
	print("--------------------")
	print("starting " + filename)
	bus = getBusinesses(bus_json, cats, filename)
	print("completed getBusinesses()")
	revs = getReviews(rev_json, bus, filename)
	print("completed getReviews()")


''' driver code '''

# get yelp json data
bus_json = readJson("../yelp/business.json")
rev_json = readJson("../yelp/small_review.json")

# get reviews for coffee
cats = ["Coffee & Tea", "Coffee Roasteries", "Coffeeshops"]
categoryReviews(bus_json, rev_json, cats, "coffee.json")

# get reviews for grocery stores
cats = ["Grocery"]
categoryReviews(bus_json, rev_json, cats, "grocery.json")

# get review for gyms
cats = ["Gyms"]
categoryReviews(bus_json, rev_json, cats, "gym.json")

