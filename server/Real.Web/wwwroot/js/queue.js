function Queue(maxLength) {
    this.maxLength = maxLength ?? Number.MAX_SAFE_INTEGER;
    this.elements = [];
}

Queue.prototype.enqueue = function(e) {
    let returnValue = null;
    if (this.elements.length >= this.maxLength) {
        returnValue = this.dequeue();
    }
    this.elements.push(e);
    return returnValue;
}

Queue.prototype.dequeue = function() {
    return (this.elements.length) ? this.elements.shift() : null;
}

Queue.prototype.peek = function() {
    return (this.elements.length) ? this.elements[0] : null;
}

Queue.prototype.length = function() {
    return this.elements.length;
}