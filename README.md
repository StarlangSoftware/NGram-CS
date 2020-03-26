# NGram-CS

Detailed Description
============
+ [Training NGram](#training-ngram)
+ [Using NGram](#using-ngram)
+ [Saving NGram](#saving-ngram)
+ [Loading NGram](#loading-ngram)

## Training NGram
     
Boş bir NGram modeli oluşturmak için

	NGram(int N)

Örneğin,

	a = NGram(2);

boş bir bigram modeli oluşturulmaktadır.

NGram'a bir cümle eklemek için

	void AddNGramSentence(Symbol[] symbols)

Örneğin,

	string[] text1 = {"ali", "topu", "at", "mehmet", "ayşe", "gitti"};
	string[] text2 = {"ali", "top", "at", "ayşe", "gitti"};
	nGram = new NGram<String>(2);
	nGram.AddNGramSentence(text1);
	nGram.AddNGramSentence(text2);

satırları ile boş bir bigram oluşturulup, text1 ve text2 cümleleri bigram modeline 
eklenir.

NoSmoothing sınıfı smoothing için kullanılan en basit tekniktir. Eğitim gerektirmez, sadece
sayaçlar kullanılarak olasılıklar hesaplanır. Örneğin verilen bir NGram'ın NoSmoothing ile 
olasılıklarının hesaplanması için

	a.CalculateNGramProbabilities(new NoSmoothing());

LaplaceSmoothing sınıfı smoothing için kullanılan basit bir yumuşatma tekniğidir. Eğitim 
gerektirmez, her sayaca 1 eklenerek olasılıklar hesaplanır. Örneğin verilen bir NGram'ın 
LaplaceSmoothing ile olasılıklarının hesaplanması için

	a.CalculateNGramProbabilities(new LaplaceSmoothing());

GoodTuringSmoothing sınıfı smoothing için kullanılan eğitim gerektirmeyen karmaşık bir 
yumuşatma tekniğidir. Verilen bir NGram'ın GoodTuringSmoothing ile olasılıklarının 
hesaplanması için

	a.CalculateNGramProbabilities(new GoodTuringSmoothing());

AdditiveSmoothing sınıfı smoothing için kullanılan eğitim gerektiren bir yumuşatma 
tekniğidir.

	a.CalculateNGramProbabilities(new AdditiveSmoothing());

## Using NGram

Bir NGram'ın olasılığını bulmak için

	double getProbability(params Symbol[] symbols)

Örneğin, bigram olasılığını bulmak için

	a.GetProbability("ali", "topu")

trigram olasılığını bulmak için

	a.GetProbability("ali", "topu", "at")

## Saving NGram
    
NGram modelini kaydetmek için

	void SaveAsText(string fileName)

Örneğin, a modelini "model.txt" dosyasına kaydetmek için

	a.SaveAsText("model.txt");              

## Loading NGram            

Var olan bir NGram modelini yüklemek için

	NGram(string fileName)

Örneğin,

	a = NGram("model.txt");

model.txt dosyasında bulunan bir NGram modelini yükler.
