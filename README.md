N-Gram
============

An N-gram is a sequence of N words: a 2-gram (or bigram) is a two-word sequence of words like “lütfen ödevinizi”, “ödevinizi çabuk”, or ”çabuk veriniz”, and a 3-gram (or trigram) is a three-word sequence of words like “lütfen ödevinizi çabuk”, or “ödevinizi çabuk veriniz”.

## Smoothing

To keep a language model from assigning zero probability to unseen events, we’ll have to shave off a bit of probability mass from some more frequent events and give it to the events we’ve never seen. This modification is called smoothing or discounting.

### Laplace Smoothing

The simplest way to do smoothing is to add one to all the bigram counts, before we normalize them into probabilities. All the counts that used to be zero will now have a count of 1, the counts of 1 will be 2, and so on. This algorithm is called Laplace smoothing.

### Add-k Smoothing

One alternative to add-one smoothing is to move a bit less of the probability mass from the seen to the unseen events. Instead of adding 1 to each count, we add a fractional count k. This algorithm is therefore called add-k smoothing.

Video Lectures
============

[<img src="https://github.com/StarlangSoftware/NGram/blob/master/video1.jpg" width="50%">](https://youtu.be/oNWKVUdPUJY)[<img src="https://github.com/StarlangSoftware/NGram/blob/master/video2.jpg" width="50%">](https://youtu.be/ZG5m6OFdudI)

For Developers
============

You can also see [Java](https://github.com/starlangsoftware/NGram), [Python](https://github.com/starlangsoftware/NGram-Py), [Cython](https://github.com/starlangsoftware/NGram-Cy), [Swift](https://github.com/starlangsoftware/NGram-Swift), [Js](https://github.com/starlangsoftware/NGram-Js), or [C++](https://github.com/starlangsoftware/NGram-CPP) repository.

## Requirements

* C# Editor
* [Git](#git)

### Git

Install the [latest version of Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Download Code

In order to work on code, create a fork from GitHub page. 
Use Git for cloning the code to your local or below line for Ubuntu:

	git clone <your-fork-git-link>

A directory called NGram-CS will be created. Or you can use below link for exploring the code:

	git clone https://github.com/starlangsoftware/NGram-CS.git

## Open project with Rider IDE

To import projects from Git with version control:

* Open Rider IDE, select Get From Version Control.

* In the Import window, click URL tab and paste github URL.

* Click open as Project.

Result: The imported project is listed in the Project Explorer view and files are loaded.


## Compile

**From IDE**

After being done with the downloading and opening project, select **Build Solution** option from **Build** menu. After compilation process, user can run NGram-CS.

Detailed Description
============

+ [Training NGram](#training-ngram)
+ [Using NGram](#using-ngram)
+ [Saving NGram](#saving-ngram)
+ [Loading NGram](#loading-ngram)

## Training NGram
     
Boş bir NGram modeli oluşturmak için
To create an empty NGram model:

	NGram(int N)

For example,

	a = NGram(2);

this creates an empty NGram model.

To add an sentence to NGram:

	void AddNGramSentence(Symbol[] symbols)

For example,

	string[] text1 = {"jack", "read", "books", "john", "mary", "went"};
	string[] text2 = {"jack", "read", "books", "mary", "went"};
	nGram = new NGram<String>(2);
	nGram.AddNGramSentence(text1);
	nGram.AddNGramSentence(text2);

with the lines above, an empty NGram model is created and the sentences text1 and text2 are
added to the bigram model.

NoSmoothing class is the simplest technique for smoothing. It doesn't require training.
Only probabilities are calculated using counters. For example, to calculate the probabilities
of a given NGram model using NoSmoothing:

	a.CalculateNGramProbabilities(new NoSmoothing());

LaplaceSmoothing class is a simple smoothing technique for smoothing. It doesn't require
training. Probabilities are calculated adding 1 to each counter. For example, to calculate
the probabilities of a given NGram model using LaplaceSmoothing:

	a.CalculateNGramProbabilities(new LaplaceSmoothing());

GoodTuringSmoothing class is a complex smoothing technique that doesn't require training.
To calculate the probabilities of a given NGram model using GoodTuringSmoothing:

	a.CalculateNGramProbabilities(new GoodTuringSmoothing());

AdditiveSmoothing class is a smoothing technique that requires training.

	a.CalculateNGramProbabilities(new AdditiveSmoothing());

## Using NGram

To find the probability of an NGram:

	double getProbability(params Symbol[] symbols)

For example, to find the bigram probability:

	a.GetProbability("jack", "reads")

To find the trigram probability:

	a.GetProbability("jack", "reads", "books")

## Saving NGram
    
To save the NGram model:

	void SaveAsText(string fileName)

For example, to save model "a" to the file "model.txt":

	a.SaveAsText("model.txt");              

## Loading NGram            

To load an existing NGram model:

	NGram(string fileName)

For example,

	a = NGram("model.txt");

this loads an NGram model in the file "model.txt".
