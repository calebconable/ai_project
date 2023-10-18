# AI Project Outline
We will employ three distinct algorithms to assist us in determining whether fetal health is normal, suspect, or pathological. The data utilized originates from Kaggle and comprises observations extracted from cardiotocogram exams (CTGs). Leveraging a dataset consisting of 2126 rows, our objective is to interpret the available features—including fetal heart rate (FHR), fetal movements, and uterine contractions—to classify fetal health into one of the three expert categories identified by obstetricians.

The selected algorithms for this task are unsupervised k-NN (k-Nearest Neighbors), Feedforward Neural Network, and, finally, a combination of random forest and decision tree approaches. These algorithms were chosen due to their intriguing nature and our desire to compare their performances. Moreover, we deliberately opted for unsupervised k-NN given that the other algorithms operate under supervised learning, to discern the distinctions in their operational dynamics.

## Algorithms:
- Feed-forward Neural Network
- Random Forest and Decision Tree
- Outlier Detection
- k-Means
- Unsupervised k-NN

---
## Attributes / Features to Include for optimization
Features: 
- prolonged_decelerations
- abnormal_short_term_variability
- percentage_of_time_with_abnormal_long_term_variability
- accelerations
- fetal_health

---
### **Analysis of k-values for k-Means Algorithm**

When deciding on the best **`k`** for k-means clustering, it's crucial to consider both the overall accuracy and the accuracy of individual classes. This is especially important if certain classes carry more weight or if there are imbalances among the classes.

### Class-Wise Accuracy:

| K-Value | Normal Accuracy | Suspect  Accuracy | Pathological Accuracy | Overall Accuracy |
|---------|------------------|------------------|------------------|------------------|
| 1       | 100.00%          | 0.00%            | 0.00%            | 77.70%           |
| 2       | 91.84%           | 61.02%           | 0.00%            | 79.81%           |
| 3       | 91.84%           | 61.02%           | 0.00%            | 79.81%           |
| 4       | 89.43%           | 67.80%           | 0.00%            | 78.87%           |
| 5       | 89.43%           | 67.80%           | 0.00%            | 78.87%           |
| 6       | 92.45%           | 55.93%           | 19.44%           | 81.22%           |
| 7       | 94.56%           | 59.32%           | 19.44%           | 83.33%           |
| ...      | ...           | ...         | ...           | ...           |
| 12      | 95.47%           | 62.71%           | 16.67%           | 84.27%           |
| ...      | ...           | ...         | ...           | ...           |
| 15      | 93.96%           | 67.80%           | 16.67%           | 83.80%           |
| ...      | ...           | ...         | ...           | ...           |
| 20      | 93.35%           | 77.97%           | 16.67%           | 84.74%           |
| ...      | ...           | ...         | ...           | ...           |
| 29      | 94.26%           | 76.27%           | 25.00%           | 85.92%           |
| ...      | ...           | ...         | ...           | ...           |
| 34      | 96.37%           | 69.49%           | 27.78%           | 86.85%           |
| ...      | ...           | ...         | ...           | ...           |
| 166     | 94.56%           | 67.80%           | 66.67%           | 88.50%           |

- **Class 1**:
    - **`k = 34`** provides the **highest** accuracy.
    - **`k = 166`** and **`k = 29`** are slightly lower, with comparable accuracies.
- **Class 2**:
    - **`k = 29`** achieves the **highest** accuracy.
    - **`k = 34`** comes in next.
    - **`k = 166`** yields the lowest accuracy for this class.
- **Class 3**:
    - **`k = 166`** stands out with a **significantly higher** accuracy.
    - Both **`k = 34`** and **`k = 29`** lag behind, with **`k = 34`** slightly ahead of **`k = 29`**.

### Overall Accuracy:

- **`k = 166`** boasts the **highest** overall accuracy.
- **`k = 34`** trails closely behind.
- **`k = 29`** is at the third spot.

### Recommendations:

1. For those primarily concerned with **overall accuracy**, **`k = 166`** emerges as the top choice.
2. If precision in **Class 3** is paramount, then **`k = 166`** remains unbeatable.
3. For those leaning towards **interpretability, computational efficiency, and simpler model representation**, **`k = 34`** strikes a commendable balance between the number of clusters and performance.

However, it's worth noting that opting for a large number of clusters (e.g., 166) may demand more computational resources. It could also pose challenges in terms of interpretation and might run the risk of overfitting, especially if the model isn't validated with diverse datasets.

**Conclusion**:
Based on the data at hand, **`k = 166`** would be the choice for maximum accuracy. But for a blend of simplicity and performance, **`k = 34`** is recommended. Always ensure to validate the model across varied datasets to vouch for its robustness.

---
## k-NN Algorithm Selection Analysis

**Using Euclidean Distance and Weighted Sum (Full Feature Set)**

| K-Value | Execution Time (ms) | Normal Accuracy | Suspect  Accuracy | Pathological Accuracy | Overall Accuracy |
|---------|---------------------|------------------|------------------|------------------|------------------|
| 1 | 7355 | 95.77% | 59.32% | 83.33% | 89.67% |
| 2 | 7086 | 95.77% | 59.32% | 83.33% | 89.67% |
| 3 | 7059 | 95.77% | 62.71% | 83.33% | 90.14% |
| 4 | 7085 | 96.07% | 62.71% | 83.33% | 90.38% |
| 5 | 7140 | 96.07% | 62.71% | 83.33% | 90.38% |
| 6 | 7060 | 95.77% | 64.41% | 83.33% | 90.38% |
| 7 | 7054 | 96.37% | 62.71% | 83.33% | 90.61% |
| 8 | 7075 | 96.98% | 57.63% | 83.33% | 90.38% |
| 9 | 7041 | 96.68% | 55.93% | 77.78% | 89.44% |

**Using Euclidean Distance and Weighted Sum with 5 Features**

| K-Value | Execution Time (ms) | Normal Accuracy | Suspect  Accuracy | Pathological Accuracy | Overall Accuracy |
|---------|---------------------|------------------|------------------|------------------|------------------|
| 1 | 6584 | 96.98% | 83.05% | 88.89% | 94.37% |
| 2 | 6093 | 96.98% | 83.05% | 88.89% | 94.37% |
| 3 | 6133 | 94.56% | 77.97% | 91.67% | 92.02% |
| 4 | 6238 | 96.37% | 76.27% | 83.33% | 92.49% |
| 5 | 6185 | 96.98% | 77.97% | 83.33% | 93.19% |
| 6 | 6113 | 96.68% | 77.97% | 83.33% | 92.96% |
| 7 | 6093 | 96.37% | 77.97% | 88.89% | 93.19% |
| 8 | 6136 | 95.77% | 77.97% | 83.33% | 92.25% |
| 9 | 6135 | 96.37% | 72.88% | 88.89% | 92.49% |
| 10 | 6104 | 96.37% | 74.58% | 83.33% | 92.25% |
| 11 | 6184 | 96.37% | 76.27% | 80.56% | 92.25% |


(Note: The full table for the 5-features set is shortened for brevity.)

Recommendations:
If you're focusing on the best overall accuracy with the full feature set, K = 7 offers the highest overall accuracy of 90.61%.

When working with the selected 5 features, K = 1 or K = 2 provide the highest overall accuracy at 94.37%. These values of K significantly outperform the full feature set in terms of overall accuracy and also have relatively quick execution times.

It's crucial to consider both individual class accuracies and overall accuracy when making a decision. In scenarios where all classes are equally important, the overall accuracy is a reliable indicator. However, if certain classes are more critical than others, you might want to prioritize those class accuracies.

The execution time does vary with the k-values, but the variations are not significant enough to heavily impact the decision. Still, in real-time or time-sensitive applications, this can be a factor to consider.

Conclusion:
For the full feature set, a K value of 7 is recommended. When using the 5-feature set, a K value of 1 or 2 is ideal, given their superior overall accuracies.
